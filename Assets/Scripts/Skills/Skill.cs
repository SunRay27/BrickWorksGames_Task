using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object which contains info about player skill
/// </summary>
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; private set; } = "Sample skill";


    [field: SerializeField]
    public int LearnCost { get; private set; } = 1;


    [field: SerializeField]
    public Sprite Sprite { get; private set; }

    /// <summary>
    /// Should contain all neighbor skills (all skills which are connected with this one)
    /// </summary>
    [field: SerializeField]
    public List<Skill> ConnectedSkills { get; private set; } = new List<Skill>();

}
