using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Subject
{
  public string name;
  public GridNode gridLocation;
  public static List<Subject> globalSubjects = new List<Subject>();
  public List<string> positiveOpinionStrings = new List<string>();
  public List<string> neutralOpinionStrings = new List<string>();
  public List<string> negativeOpinionStrings = new List<string>();

  Subject()
  {

  }
}
