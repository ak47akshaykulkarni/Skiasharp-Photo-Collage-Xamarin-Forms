using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCollage.Interfaces
{
    public interface IImageStorageService
    {
        Task<bool> SaveBitmap(byte[] bitmapData, string filename);
    }
}
