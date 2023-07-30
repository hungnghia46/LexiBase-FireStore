using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
        [HttpGet("Get_All_Account")]
        public async Task<IActionResult> Get()
        {
            CollectionReference collectionRef = db.Collection("accounts");
            QuerySnapshot snapshots = await collectionRef.GetSnapshotAsync();

            List<Account> accounts = new List<Account>();
            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Account account = doc.ConvertTo<Account>();
                    accounts.Add(account);
                }
            }
            if (accounts.Count > 0)
            {
                return Ok(accounts);
            }
            else { return NotFound("Account is Empty"); }
        }

        [HttpGet("Get acc by Email")]
        public async Task<IActionResult> GetByEmail(String email)
        {
            CollectionReference collectionRef = db.Collection("accounts");
            Query query = collectionRef.WhereEqualTo("Email", email);
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            if (snapshots.Count > 0)
            {
                DocumentSnapshot doc = snapshots.Documents.First();
                Account acc = doc.ConvertTo<Account>();
                return Ok(acc);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("Add new Account")]
        public async Task<IActionResult> addAccount(Account acc)
        {
            try
            {
                CollectionReference collectionRef = db.Collection("accounts");
                DocumentReference document = collectionRef.Document();
                Account account = new Account()
                {
                    Name = acc.Name,
                    Email = acc.Email + "@gmail.com",
                    Password = acc.Password,
                };
                await document.SetAsync(account);

                return Ok("Add sucessfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error adding the account:" + ex.Message);
            }
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
        [HttpPut("Update by Email{Email}")]
        public async Task<IActionResult> UpdateById(String Email, AccountView accountView)
        {

            Query query = db.Collection("accounts").WhereEqualTo("Email", Email);
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            DocumentSnapshot document = snapshots.Documents.First();

            DocumentReference documentRef = document.Reference;
            Account account = document.ConvertTo<Account>();

            if (accountView.Name != null || accountView.Password != null)
            {
                // Only update if it has changed
                account.Name = accountView.Name;
                account.Password = accountView.Password;

            }


            await documentRef.SetAsync(account);


            //Account account = document.ConvertTo<Account>();

            return Ok(account);
        }
        [HttpPost]
        public async Task<IActionResult> insertWord(String wordName, string definition)
        {
            CollectionReference collectionRef = db.Collection("Dictionary");
            DocumentReference document = collectionRef.Document();
            Word newWord = new Word
            {
                WordName = wordName,
                Definition = definition
            };
            await document.SetAsync(newWord);
            return Ok(newWord);
        }
        [HttpGet]
        public async Task<IActionResult> searchWord(String wordname)
        {
            Query query = db.Collection("Dictionary").WhereEqualTo("word", wordname);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            List<Word> words = new List<Word>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Word word = document.ConvertTo<Word>();
                words.Add(word);
            }
            if (words.Count > 0)
            {
                return Ok(words);
            }
            return BadRequest("Not found");
        }
    }
}