using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModel;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecxiController : ControllerBase
    {
        FirestoreDb db;
        public LecxiController()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"lexibase.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("lexibase-bf6c0");
        }
        [HttpGet("Get All Account")]
        public async Task<IActionResult> Get()
        {
            CollectionReference collectionRef = db.Collection("accounts");
            QuerySnapshot snapshots = await collectionRef.GetSnapshotAsync();

            List<Dictionary<string, object>> accountsData = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Dictionary<String, object> data = doc.ToDictionary();
                    accountsData.Add(data);
                }
            }
            if (accountsData.Count > 0)
            {
                return Ok(accountsData);
            }
            else { return NotFound("Account is Empty"); }
        }
        [HttpGet("Get acc by id")]
        public async Task<IActionResult> GetAll(Guid id)
        {
            CollectionReference collectionRef = db.Collection("accounts");
            DocumentReference DOC = collectionRef.Document(id.ToString());
            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<String, object> data = snapshot.ToDictionary();
                return Ok(data);
            }
            else
            {
                return NotFound("Account not found!"); // Document with the specified ID not found
            }
        }
        [HttpGet("Get acc by Email")]
        public async Task<IActionResult> GetByEmail(String email)
        {
            CollectionReference collectionRef = db.Collection("accounts");
            Query query = collectionRef.WhereEqualTo("Email", email);
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            if (snapshots.Count > 0)
            {
                DocumentSnapshot doc = snapshots.First();
                Dictionary<string, object> data = doc.ToDictionary();
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("Add new Account")]
        public async Task<IActionResult> test(AccountView accView)
        {
            Account acc = new Account();
            CollectionReference collectionRef = db.Collection("accounts");
            acc.Id = Guid.NewGuid();
            DocumentReference DOC = collectionRef.Document(acc.Id.ToString());
            Dictionary<String, object> data = new Dictionary<String, object>()
            {
                { "Name", accView.Name },
                { "Email", accView.Email },
                { "Password", accView.Password }
            };
            await DOC.SetAsync(data);
            return Ok(data);
        }
        [HttpDelete("Delete by id")]
        public async Task<IActionResult> Delete(String email)
        {
            CollectionReference collectionRef = db.Collection("accounts");
            Query query = collectionRef.WhereEqualTo("Email", email);
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            if (snapshots.Count > 0)
            {
                DocumentSnapshot doc = snapshots.First();
                await doc.Reference.DeleteAsync();
                return Ok("Delete successfully!");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut("Update by id")]
        public async Task<IActionResult> UpdateById(Guid id, AccountView updatedAccountData)
        {
            CollectionReference collectionRef = db.Collection("accounts");
            DocumentReference docRef = collectionRef.Document(id.ToString());

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "Name", updatedAccountData.Name },
            { "Email", updatedAccountData.Email },
            { "Password", updatedAccountData.Password }
        };

                await docRef.SetAsync(data);
                return Ok("Account updated successfully!");
            }
            else
            {
                return NotFound("Account not found!"); // Document with the specified ID not found
            }
        }

    }
}