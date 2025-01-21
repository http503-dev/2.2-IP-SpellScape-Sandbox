using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class Database : MonoBehaviour
{
    DatabaseReference mDatabaseRef;

    private void Start()
    {
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
}
