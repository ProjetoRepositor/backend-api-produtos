namespace App.Util;

public class RootObject
{
    public string description { get; set; }
    public long gtin { get; set; }
    public string thumbnail { get; set; }
    public object width { get; set; }
    public object height { get; set; }
    public object length { get; set; }
    public object net_weight { get; set; }
    public object gross_weight { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
    public object release_date { get; set; }
    public string price { get; set; }
    public double avg_price { get; set; }
    public double max_price { get; set; }
    public double min_price { get; set; }
    public Gtins[] gtins { get; set; }
    public string origin { get; set; }
    public string barcode_image { get; set; }
    public Brand brand { get; set; }
    public Gpc gpc { get; set; }
    public Ncm ncm { get; set; }
    public Cest cest { get; set; }
}

public class Gtins
{
    public long gtin { get; set; }
    public Commercial_unit commercial_unit { get; set; }
}

public class Commercial_unit
{
    public string type_packaging { get; set; }
    public int quantity_packaging { get; set; }
    public int? ballast { get; set; }
    public int? layer { get; set; }
}

public class Brand
{
    public string name { get; set; }
    public string picture { get; set; }
}

public class Gpc
{
    public string code { get; set; }
    public string description { get; set; }
}

public class Ncm
{
    public string code { get; set; }
    public string description { get; set; }
    public string full_description { get; set; }
    public object ex { get; set; }
}

public class Cest
{
    public int id { get; set; }
    public string code { get; set; }
    public string description { get; set; }
    public int parent_id { get; set; }
}