using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LexionController : ControllerBase
    {
        FirestoreDb db;
        public LexionController()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"lexibase.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("lexibase-bf6c0");
        }
        [HttpGet("Get_All_Word")]
        public async Task<IActionResult> getAllWord()
        {
            // Get a reference to the "Dictionary" collection in Firestore
            CollectionReference collectionRef = db.Collection("Dictionary");

            // Fetch all the documents (snapshots) from the "Dictionary" collection in Firestore
            QuerySnapshot snapshots = await collectionRef.GetSnapshotAsync();

            // Create a list of Word objects to store the retrieved data
            List<Word> words = snapshots.Documents
                // Filter out any documents that don't exist (just in case)
                .Where(doc => doc.Exists)
                // Convert each DocumentSnapshot to a Word object using the ConvertTo method
                .Select(doc => doc.ConvertTo<Word>())
                // Convert the IEnumerable<Word> to a List<Word>
                .ToList();

            // Check if there are words in the list
            if (words.Count > 0)
            {
                // If there are words, return an Ok response with the list of words
                return Ok(words);
            }

            // If the list is empty, return a BadRequest response with an appropriate message
            return BadRequest("Empty word list!");
        }

        [HttpGet("Search_Word/{searchTerm}")]
        public async Task<IActionResult> getAllWord(string searchTerm)
        {
            // Get a reference to the "Dictionary" collection in Firestore
            CollectionReference collectionRef = db.Collection("Dictionary");

            // Fetch all the documents (snapshots) from the "Dictionary" collection in Firestore
            QuerySnapshot snapshots = await collectionRef.GetSnapshotAsync();

            // Create a list of Word objects to store the retrieved data
            List<Word> words = snapshots.Documents
                // Filter out any documents that don't exist (just in case)
                .Where(doc => doc.Exists)
                // Convert each DocumentSnapshot to a Word object using the ConvertTo method
                .Select(doc => doc.ConvertTo<Word>())
                // Filter the Word objects based on the search term in the WordName field
                .Where(word => word.WordName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                // Convert the IEnumerable<Word> to a List<Word>
                .ToList();

            // Check if there are words in the list
            if (words.Count > 0)
            {
                // If there are words, return an Ok response with the list of words
                return Ok(words);
            }

            // If the list is empty, return a BadRequest response with an appropriate message
            return BadRequest("No matching words found!");
        }
        [HttpPost("Add_New_Word/{wordName},{definition}")]
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
        [HttpDelete("Delete_Word/{Word}")]
        public async Task<IActionResult> deleteWord(string Word)
        {
            CollectionReference collectionRef = db.Collection("Dictionary");
            Query query = collectionRef.WhereEqualTo("word", Word.ToLower());
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            if (snapshots.Documents.Count > 0)
            {
                DocumentSnapshot doc = snapshots.First();
                await doc.Reference.DeleteAsync();
                return Ok("Delete " + Word + " sucessfully!");
            }
            else
            {
                return NotFound(Word + " not found in dictionary!");
            }
        }
    }
}