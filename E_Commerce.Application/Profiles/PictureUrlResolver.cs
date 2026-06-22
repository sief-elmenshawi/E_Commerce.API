using AutoMapper;
using E_Commerce.Application.DTOs;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Profiles
{
    internal class PictureUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly UrlSettings urlSettings;

        public PictureUrlResolver(IOptions<UrlSettings> options)
        {
            urlSettings = options.Value;
        }

        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            // Source => image/products/FormalBlazer.jpg
            // Return => https://localhost:7175/Files/image/products/FormalBlazer.jpg
            var baseUrl = urlSettings.BaseUrl.TrimEnd('/');
            var path = source.PictureUrl.TrimStart('/');

            return $"{baseUrl}/Files/{path}";

        }
    }

    public class UrlSettings
    {
        public string BaseUrl { get; set; }
    }
}
