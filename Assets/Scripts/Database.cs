using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class Database : MonoBehaviour
{
    DatabaseReference mDatabaseRef;

    public static Database Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
}
