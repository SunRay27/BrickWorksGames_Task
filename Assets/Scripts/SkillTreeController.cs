using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillTreeController : MonoBehaviour
{
    //selected skill set
    [SerializeField]
    private SkillSet skillSet;

    //UI reference
    [SerializeField]
    private SkillTreeUI uiTree;

    //interact buttons
    [SerializeField]
    private Button earnPointButton, learnSkillButton, dismissSkillButton, dismissAllSkillsButton;

    //player info
    private PlayerInfo playerInfo;

    //selected skill reference
    private Skill selectedSkill = null;

    private void Start()
    {
        //init buttons
        earnPointButton.onClick.AddListener(EarnSkillPoint);
        learnSkillButton.onClick.AddListener(LearnSkill);
        dismissSkillButton.onClick.AddListener(DismissSkill);
        dismissAllSkillsButton.onClick.AddListener(DismissAllSkills);

        //player gets base skill at start
        playerInfo = new PlayerInfo();
        playerInfo.LearnedSkills.Add(skillSet.GetBaseSkill());

        //init view
        InitTree();

        //select base skill
        SelectSkill(0);
    }



    /// <summary>
    /// Initializes UI skill view
    /// </summary>
    private void InitTree()
    {
        //set controller dependency
        uiTree.Init(this);

        //map all SkillSet skills to ui layout
        uiTree.InitSkillSet(skillSet);

        //display available points
        uiTree.SetAvailablePoints(playerInfo.AvailableSkillPoints);


        //get learned skills indexes and pass them to view
        List<int> playerLearnedSkillsIndexes = new List<int>();
        foreach (var item in playerInfo.LearnedSkills)
        {
            int skillIndex = skillSet.GetSkillIndex(item);

            //this skill is not from this skill set, so skip it
            if (skillIndex == -1)
                continue;

            playerLearnedSkillsIndexes.Add(skillIndex);
        }


        uiTree.SetLearnedSkills(playerLearnedSkillsIndexes);
    }
    /// <summary>
    /// Gives player one skill point
    /// </summary>
    private void EarnSkillPoint()
    {
        //update player info
        playerInfo.AvailableSkillPoints++;

        //update view
        uiTree.SetAvailablePoints(playerInfo.AvailableSkillPoints);

        //update learn button
        learnSkillButton.interactable = CanLearnSelectedSkill();
    }
    /// <summary>
    /// Selects skill from current SkillSet
    /// </summary>
    /// <param name="skillId">Id of the skill in current SkillSet</param>
    internal void SelectSkill(int skillId)
    {
        selectedSkill = skillSet.GetSkill(skillId);

        //update view
        uiTree.SelectSkill(skillId);
        uiTree.UpdateSkillCost(selectedSkill.LearnCost);


        //enable buttons if we can perform their actions
        dismissSkillButton.interactable = CanDismissSelectedSkill();
        learnSkillButton.interactable = CanLearnSelectedSkill();
    }
    /// <summary>
    /// Adds selected skill to player
    /// </summary>
    private void LearnSkill()
    {
        if (selectedSkill == null)
            return;

        //decrese player's skill points by cost amount of the skill
        playerInfo.AvailableSkillPoints -= selectedSkill.LearnCost;
        //add skill to player's skills
        playerInfo.LearnedSkills.Add(selectedSkill);

        //update view
        uiTree.LearnSkill(skillSet.GetSkillIndex(selectedSkill));
        uiTree.SetAvailablePoints(playerInfo.AvailableSkillPoints);

        //update buttons
        learnSkillButton.interactable = false;
        dismissSkillButton.interactable = true;
    }
    /// <summary>
    /// Dismisses selected skill and returns skill points to player 
    /// </summary>
    private void DismissSkill()
    {
        if (selectedSkill == null)
            return;

        //update player info
        playerInfo.AvailableSkillPoints += selectedSkill.LearnCost;
        playerInfo.LearnedSkills.Remove(selectedSkill);

        //update view
        uiTree.DismissSkill(skillSet.GetSkillIndex(selectedSkill));
        uiTree.SetAvailablePoints(playerInfo.AvailableSkillPoints);

        //update buttons
        learnSkillButton.interactable = true;
        dismissSkillButton.interactable = false;
    }
    /// <summary>
    /// Resets all skills (except base skill from current SkillSet)
    /// </summary>
    private void DismissAllSkills()
    {
        Skill baseSkill = skillSet.GetBaseSkill();

        //update player info
        //add points for each learned skill except base skill
        foreach (var item in playerInfo.LearnedSkills)
            if (item != baseSkill)
                playerInfo.AvailableSkillPoints += item.LearnCost;

        //remove all learned skills from player info
        playerInfo.LearnedSkills.RemoveWhere(i => i != baseSkill);

        //update view
        uiTree.SetAvailablePoints(playerInfo.AvailableSkillPoints);
        uiTree.DismissAllSkills();

        //update buttons
        dismissSkillButton.interactable = CanDismissSelectedSkill();
        learnSkillButton.interactable = CanLearnSelectedSkill();
    }




    /// <summary>
    /// Returns true if player can learn selected skill
    /// </summary>
    /// <returns>asd</returns>
    private bool CanLearnSelectedSkill()
    {
        if (selectedSkill == null)
            return false;


        //check new skill cost
        if (selectedSkill.LearnCost > playerInfo.AvailableSkillPoints)
            return false;

        //check if player already has this skill
        if (playerInfo.LearnedSkills.Contains(selectedSkill))
            return false;

        //we must find path at least from one skill in learned skills
        foreach (var item in playerInfo.LearnedSkills)
            if (item.ConnectedSkills.Contains(selectedSkill))
                return true;

        return false;
    }
    /// <summary>
    /// Returns true if player can dismiss skill
    /// </summary>
    /// <returns></returns>
    private bool CanDismissSelectedSkill()
    {
        if (selectedSkill == null)
            return false;

        Skill baseSkill = skillSet.GetBaseSkill();

        if (selectedSkill == baseSkill)
            return false;

        if (!playerInfo.LearnedSkills.Contains(selectedSkill))
            return false;


        //we must find at least one path from base skill (on grapth with exlcuded skillToDismiss)
        foreach (var dismissNeighbor in selectedSkill.ConnectedSkills)
        {
            //if this node is not learned, we don't need to check it
            if (!playerInfo.LearnedSkills.Contains(dismissNeighbor))
                continue;
            //same goes for base node
            if (dismissNeighbor == baseSkill)
                continue;

            if (!IsPathAvailable(dismissNeighbor, baseSkill, selectedSkill))
                return false;

        }

        return true;
    }
    /// <summary>
    /// Returns true if at least one path is available for learned skills
    /// </summary>
    /// <param name="from">Skill to start search from</param>
    /// <param name="to">Target skill</param>
    /// <param name="exclude">Skill to be excluded from search</param>
    /// <returns></returns>
    private bool IsPathAvailable(Skill from, Skill to, Skill exclude = null)
    {
        //BFS traversal

        List<Skill> visited = new List<Skill>();
        Queue<Skill> queue = new Queue<Skill>();
        queue.Enqueue(from);
        while (queue.Count > 0)
        {
            Skill current = queue.Dequeue();
            visited.Add(current);
            foreach (var neighbor in current.ConnectedSkills)
            {
                if (neighbor == exclude)
                    continue;
                if (!playerInfo.LearnedSkills.Contains(neighbor))
                    continue;
                if (visited.Contains(neighbor))
                    continue;

                queue.Enqueue(neighbor);
                if (neighbor == to)
                    return true;
            }
        }
        return false;
    }
}
