using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using App.Models;
using App.Util;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("api/v1/[controller]")]
public class ProdutoController : ControllerBase
{
    private static AmazonDynamoDBClient client = new();
    private static DynamoDBContext context = new(client);
    
    [HttpGet]
    public async Task<IActionResult> Buscar([FromQuery] string? nome, [FromQuery] string? ean)
    {
        List<Produto> allDocs = new();
        
        if (ean != null)
        {
            foreach (var code in ean.Split(";"))
            {
                var newDoc = await BuscarProduto(code);
                
                if (newDoc != null)
                    allDocs.Add(newDoc);
            }

            return Ok(allDocs);
        }
        
        var conditions = new List<ScanCondition>();
        
        if (nome != null)
        {
            Console.WriteLine(nome);
            conditions.Add(
                new("Nome", ScanOperator.Contains, nome)
            );
        }

        allDocs = await context.ScanAsync<Produto>(conditions).GetRemainingAsync();

        var limite = Math.Min(allDocs.Count, 50);
        
        return Ok(allDocs.GetRange(0, limite));
    }

    [HttpGet("{ean}")]
    public async Task<IActionResult> BuscarPorCodigoEan([FromRoute] string ean)
    {
        var response = await BuscarProduto(ean);
        return response != null ? Ok(response) : NotFound();
    }

    private async Task<Produto?> BuscarProduto(string ean)
    {
        Console.WriteLine($"Buscando produto {ean}");

        if (!ValidarEAN(ean)) return null;
        
        var produto = await context.LoadAsync<Produto>(ean);

        if (produto is null)
        {
            Console.WriteLine($"Produto {ean} não encontrado na base, procurando na bluecosmos");
            
            string request;

            var httpUtil = new Cosmos();

            try
            {
                request = httpUtil.DownloadString($"https://api.cosmos.bluesoft.com.br/gtins/{ean}.json");
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Produto {ean} não encontrado na bluecosmos");
                
                var produtoProvisorio = new Produto
                {
                    Ean = ean,
                    CosmosImageUrl = "",
                    Nome = ean,
                };
                
                return produtoProvisorio;
            }
            
            Console.WriteLine(request);
            
            var response = JsonSerializer.Deserialize<RootObject>(request)!;
            
            var novoProduto = new Produto
            {
                Ean = ean,
                Nome = response.description,
                CosmosImageUrl = response.thumbnail,
            };

            await context.SaveAsync(novoProduto);

            Console.WriteLine($"Produto {ean} encontrado na bluecosmos");
            
            return novoProduto;
        }

        Console.WriteLine($"Produto {ean} encontrado");
        
        return produto;
    }
    
    private bool ValidarEAN(string ean)
    {
        // Verifica se a string contém apenas dígitos
        foreach (char c in ean)
        {
            if (!char.IsDigit(c))
                return false;
        }

        // Verifica se o código tem 13 dígitos
        if (ean.Length != 13)
            return false;

        int soma = 0;
        for (int i = 0; i < 12; i++)
        {
            int digito = ean[i] - '0';
            if (i % 2 == 0)
                soma += digito;
            else
                soma += digito * 3;
        }

        int resto = soma % 10;
        int digitoVerificador = (resto == 0) ? 0 : 10 - resto;
        return digitoVerificador == (ean[12] - '0');
    }
}