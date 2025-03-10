﻿namespace MauiKit.Models
{
    public class RealEstateProperty
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrincipalImage => Images.FirstOrDefault();
        public List<string> Images { get; set; }
        public Agent Agent { get; set; }
        public Category Category { get; set; }
        public string Address { get; set; }
        public List<Amenity> Amenities { get; set; }
        public int Rooms { get; set; }
        public int Bathrooms { get; set; }
        public int SquareMeters { get; set; }
        public bool HasParking { get; set; }
        public bool IsFeatured { get; set; }
        public decimal Price { get; set; }
        public int WalkScore { get; set; }
        public int TransitScore { get; set; }
        public int BikeScore { get; set; }
    }
    public class Amenity
    {
        public string Icon { get; set; }
        public string Name { get; set; }
    }
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
}
