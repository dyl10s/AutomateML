using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Objects;

namespace Website.Models
{
    [PrimaryKey("FileStoreId")]
    public class FileStore
    {
        public int FileStoreId { get; set; }
        public byte[] Data { get; set; }

        public static ReturnResult<FileStore> InsertUpdate(IDatabase db, FileStore file)
        {
            var results = new ReturnResult<FileStore>();

            try
            {
                if(file.FileStoreId == 0)
                {
                    db.Insert(file);
                }
                else
                {
                    db.Update(file);
                }

                results.Success = true;
                results.Item = file;
            }
            catch(Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error saving the file.";
                results.Exception = e;
            }

            return results;
        }

        public static ReturnResult<FileStore> GetById(IDatabase db, int Id)
        {
            var results = new ReturnResult<FileStore>();

            try
            {
                results.Item = db.SingleById<FileStore>(Id);
                results.Success = true;
            }
            catch(Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error getting the file.";
                results.Exception = e;
            }

            return results;
        }
    }
}
