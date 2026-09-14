using E_Commerce.Application.Common;
using Xunit;

namespace E_Commerce.Tests.Common
{
    public class ProductQueryParamsTests
    {
        [Fact]
        public void PageIndex_LessThanOne_ClampsToOne()
        {
            var sut = new ProductQueryParams { PageIndex = 0 };

            Assert.Equal(1, sut.PageIndex);
        }

        [Fact]
        public void PageIndex_Negative_ClampsToOne()
        {
            var sut = new ProductQueryParams { PageIndex = -5 };

            Assert.Equal(1, sut.PageIndex);
        }

        [Fact]
        public void PageIndex_ValidValue_IsPreserved()
        {
            var sut = new ProductQueryParams { PageIndex = 3 };

            Assert.Equal(3, sut.PageIndex);
        }

        [Fact]
        public void PageSize_GreaterThanMax_ClampsToMax()
        {
            var sut = new ProductQueryParams { PageSize = 999 };

            Assert.Equal(10, sut.PageSize);
        }

        [Fact]
        public void PageSize_LessThanOne_ResetsToDefault()
        {
            var sut = new ProductQueryParams { PageSize = 0 };

            Assert.Equal(5, sut.PageSize);
        }

        [Fact]
        public void PageSize_ValidValue_IsPreserved()
        {
            var sut = new ProductQueryParams { PageSize = 2 };

            Assert.Equal(2, sut.PageSize);
        }

        [Fact]
        public void DefaultValues_PageIndexOneAndPageSizeFive()
        {
            var sut = new ProductQueryParams();

            Assert.Equal(1, sut.PageIndex);
            Assert.Equal(5, sut.PageSize);
        }
    }
}
