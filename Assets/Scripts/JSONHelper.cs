using UnityEngine;
using System;

// Class used to parse a JSON array that is received from the webrequest in the VideoController class
// Needed because the default JSONUtility class can't parse arrays of JSON objects
// Main concept is to wrap the JSON array into one JSON object with an array of properties so that the JSONUtility can parse it
public class JSONHelper : MonoBehaviour
{
    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }

    public static string fixJson(string value) {
        // Add this string to the reponse received from the server in order to parse it as a single JSON object that contains an array of properties
        value = "{\"Items\":" + value + "}";
        return value;
    }
}
