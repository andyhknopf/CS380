using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TownLocationData : MonoBehaviour
{
  // WARNING! ONLY COLLECT THIS DATA ONCE AT PROGRAM START
  private CityNewsListener [] cityNewsListeners;
  public Dictionary<string, Vector3> townLookupTable = new Dictionary<string, Vector3>();

  private void Awake()
  {
    //cityNewsListeners
  }

  // Update is called once per frame
  void Update()
  {
        
  }
}
