using Google.Cloud.Firestore;

[FirestoreData]
public class Account
{
    [FirestoreProperty("Name")]
    public string Name { get; set; }
    [FirestoreProperty("Email")]
    public string Email { get; set; }
    [FirestoreProperty("Password")]
    public string Password { get; set; }
}
