namespace App.Util;

public class RootObject
{
    public string description { get; set; }
    public string thumbnail { get; set; }
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