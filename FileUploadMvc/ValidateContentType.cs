using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FileUploadMvc
{
    public class ValidateContentType
    {
        //private string EnsureCorrectFilename(string filename)
        //{
        //    if (filename.Contains("\\"))
        //        filename = filename.Substring(filename.LastIndexOf("\\") + 1);

        //    return filename;
        //}

        //private string GetPathAndFilename(string filename)
        //{
        //    return this.hostingEnvironment.WebRootPath + "\\uploads\\" + filename;
        //}

        public static string GetMimeType(byte[] content)
        {
            string output = string.Empty;
            int MaxContent = content.Length;
            string mime = string.Empty; int result = 0;
            byte[] buf = new byte[MaxContent];
            //content.InputStream.Read(document, 0, file.ContentLength);
            System.UInt32 mimetype;
            FindMimeFromData(0, null, buf, 256, null, 0, out mimetype, 0);
            System.IntPtr mimeTypePtr = new IntPtr(mimetype);
            mime = Marshal.PtrToStringUni(mimeTypePtr);
            Marshal.FreeCoTaskMem(mimeTypePtr);

            if (mime == "application/pdf")
            {
                // upload the File because file is valid  
                result = 1;
                output = "Valid";
            }
            else
            {
                //  file is Invalid  
                result = 0;
                output = "Invalid";
            }
            return output;
        }

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(uint pBC,
        [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        uint cbSize, [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
        uint dwMimeFlags,
        out uint ppwzMimeOut,
        uint dwReserverd);
    }
}
