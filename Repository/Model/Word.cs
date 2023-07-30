using Google.Cloud.Firestore;

[FirestoreData]
public class Word
{
    [FirestoreProperty("word")] // Specify the field name for Firestore
    public string WordName { get; set; }
    [FirestoreProperty("definition")] // Specify the field name for Firestore
    public string Definition { get; set; }
}
