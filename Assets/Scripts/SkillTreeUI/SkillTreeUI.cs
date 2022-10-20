using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SkillTreeUI is a component responsible for skills UI representation
/// </summary>
public class SkillTreeUI : MonoBehaviour
{
    //RectTransform of selection effect object
    [SerializeField]
    private RectTransform selectorTransform;

    //info texts
    [SerializeField]
    private TextMeshProUGUI playerPointsText, skillCostText;

    //a dictionary of <skillIndex, UI representation> according to init SkillSet
    private Dictionary<int, SkillButton> skillButtons = new Dictionary<int, SkillButton>();

    private SkillTreeController controller;

    /// <summary>
    /// Sets skill tree controller reference
    /// </summary>
    /// <param name="skillTreeController">An instance of SkillTreeController</param>
    public void Init(SkillTreeController skillTreeController)
    {
        controller = skillTreeController;
    }

    /// <summary>
    /// Fills UI elements according to certain SkillSet's skills
    /// </summary>
    /// <param name="skillSet">SkillSet to be mapped to</param>
    /// <exception cref="ArgumentException">Throws if SkillSet and UI layout sizes don't match </exception>
    public void InitSkillSet(SkillSet skillSet)
    {
        //can be moved to Init, but i guess it doesn't matter that much
        SkillButton[] buttons = GetComponentsInChildren<SkillButton>();

        //check sizes
        if (skillSet.SkillCount != buttons.Length)
            throw new ArgumentException($"Skillset skill count doesn't match with tree layout: set has {skillSet.SkillCount}, layout has {skillButtons.Count}");

        skillButtons.Clear();

        //register all buttons
        for (int i = 0; i < skillSet.SkillCount; i++)
            skillButtons.Add(buttons[i].SkillPlacementId, buttons[i]);

        //fill all buttons with skill info according to SkillPlacementId
        foreach (var skillButton in skillButtons)
            skillButton.Value.Init(skillSet.GetSkill(skillButton.Key), controller);

    }

    /// <summary>
    /// Sets all buttons with corresponding skill ids to learned state
    /// </summary>
    /// <param name="playerLearnedSkillIds"></param>
    public void SetLearnedSkills(List<int> playerLearnedSkillIds)
    {
        //clear all previous results
        //DismissAllSkills();

        foreach (int skillId in playerLearnedSkillIds)
            skillButtons[skillId].SetLearnedState(true);
    }

    /// <summary>
    /// Moves selector RectTransform to button with corresponding skill info
    /// </summary>
    /// <param name="skillId"></param>
    public void SelectSkill(int skillId)
    {
        //move selector
        RectTransform skillButtonRect = skillButtons[skillId].GetComponent<RectTransform>();
        selectorTransform.anchoredPosition = skillButtonRect.anchoredPosition;
        selectorTransform.sizeDelta = skillButtonRect.sizeDelta * 1.05f;
    }

    /// <summary>
    /// Sets available skill points text
    /// </summary>
    /// <param name="newValue">Value of available skill points</param>
    public void SetAvailablePoints(int newValue)
    {
        playerPointsText.text = $"Available points: {newValue}";
    }

    /// <summary>
    /// Sets currently selected skill's learn cost text
    /// </summary>
    /// <param name="newValue">New cost</param>
    public void UpdateSkillCost(int newValue)
    {
        skillCostText.text = $"Selected skill cost: {newValue}";
    }
    /// <summary>
    /// Sets skill button state to LEARNED
    /// </summary>
    /// <param name="skillId">Skill id</param>
    public void LearnSkill(int skillId)
    {
        skillButtons[skillId].SetLearnedState(true);
    }
    /// <summary>
    /// Sets skill button state to UNLEARNED
    /// </summary>
    /// <param name="skillId">Skill id</param>
    public void DismissSkill(int skillId)
    {
        skillButtons[skillId].SetLearnedState(false);
    }

    /// <summary>
    /// Sets ALL skill buttons state to UNLEARNED
    /// (Except for base skill)
    /// </summary>
    public void DismissAllSkills()
    {
        foreach (var item in skillButtons)
            if (item.Key != 0)
                item.Value.SetLearnedState(false);
    }


}
