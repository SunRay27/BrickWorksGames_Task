using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object which contains all skills from one skill set
/// It gives ability to change SkillSet in SkillTreeController and connect it with another view layouts in future
/// 
/// In SkillSet base skill always has 0 index and learnable skills start with 1
/// </summary>
[CreateAssetMenu(fileName = "NewSkillSet", menuName = "Skill set")]
public class SkillSet : ScriptableObject
{
    [SerializeField]
    private Skill baseSkill;

    [SerializeField]
    private List<Skill> skills;

    public int SkillCount => skills.Count + 1;


    public int GetSkillIndex(Skill skill)
    {
        int index = skills.IndexOf(skill);
        if (index != -1)
            return index + 1;
        else if (skill == baseSkill)
            return 0;
        else
            return -1;
    }
    public Skill GetSkill(int index)
    {
        return index == 0 ? baseSkill : skills[index - 1];
    }

    public Skill GetBaseSkill()
    {
        return baseSkill;
    }
}
