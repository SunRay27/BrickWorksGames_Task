using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerInfo represents player model
/// </summary>
public class PlayerInfo
{
    public int AvailableSkillPoints { get; set; } = 0;
    public HashSet<Skill> LearnedSkills { get; set; } = new HashSet<Skill>();
}
