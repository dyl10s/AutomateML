using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Objects;

namespace Website.Models
{
    [PrimaryKey("ModelVoteId")]
    public class ModelVote
    {
        public int ModelVoteId { get; set; }
        public int ModelId { get; set; }
        public int AccountId { get; set; }

        public static ReturnResult<int> GetModelVotes(IDatabase db, int modelId)
        {
            var results = new ReturnResult<int>();

            try
            {
                var votes = db.Single<int>("SELECT COUNT(*) FROM ModelVote WHERE ModelId = @0", modelId);
                results.Item = votes;
                results.Success = true;
            }
            catch(Exception e)
            {
                results.ErrorMessage = "There was an error loading the model votes";
                results.Exception = e;
                results.Success = false;
            }

            return results;
        }

        public static ReturnResult<bool> HasVotedForModel(IDatabase db, int accountId, int modelId)
        {
            var results = new ReturnResult<bool>();

            try
            {
                //Check if our vote exists
                var votes = db.Single<int>("SELECT COUNT(*) FROM ModelVote WHERE ModelId = @0 AND AccountId = @1", modelId, accountId);
                if (votes == 0)
                {
                    results.Item = false;
                }
                else
                {
                    results.Item = true;
                }

                results.Success = true;
            }
            catch (Exception e)
            {
                results.ErrorMessage = "There was an error loading the model votes";
                results.Exception = e;
                results.Success = false;
            }

            return results;
        }

        //Returns true if we are now in the voted state and false if we are now in the non voted state
        public static ReturnResult<bool> ToggleVote(IDatabase db, int accountId, int modelId)
        {
            var results = new ReturnResult<bool>();

            try
            {
                //Check if our vote exists
                var votes = db.Single<int>("SELECT COUNT(*) FROM ModelVote WHERE ModelId = @0 AND AccountId = @1", modelId, accountId);
                if(votes == 0)
                {
                    db.Insert(new ModelVote()
                    {
                        AccountId = accountId,
                        ModelId = modelId
                    });
                    results.Item = true;
                }
                else
                {
                    db.Execute("DELETE FROM ModelVote WHERE ModelId = @0 AND AccountId = @1", modelId, accountId);
                    results.Item = false;
                }
                
                results.Success = true;
            }
            catch (Exception e)
            {
                results.ErrorMessage = "There was an error loading the model votes";
                results.Exception = e;
                results.Success = false;
            }

            return results;
        }
    }
}
