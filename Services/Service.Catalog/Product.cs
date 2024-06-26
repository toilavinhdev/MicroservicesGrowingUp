﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Service.Catalog;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;
    
    public string Summary { get; set; } = default!;
    
    public string Description { get; set; } = default!;
    
    public string ImageFile { get; set; } = default!;
    
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
}