using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace App2.Business.Abstracts
{
    public interface IResizeImage
    {
        Task<byte[]> ResizeImage(byte[] imageData, float width, float height);
    }
}
