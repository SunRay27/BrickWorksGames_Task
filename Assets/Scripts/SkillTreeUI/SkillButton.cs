using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI representation of Skill
/// </summary>
public class SkillButton : MonoBehaviour, IPointerClickHandler
{

    /// <summary>
    /// Determines which skill will be placed in this button (should match with SkillSet.GetSkillIndex())
    /// </summary>
    [field: SerializeField]
    public int SkillPlacementId { get; private set; } = 0;


    [SerializeField]
    private Image skillFrame, skillSpriteImage;
    [SerializeField]
    private TextMeshProUGUI nameText;


    //ref to controller
    private SkillTreeController skillTreeController;


    /// <summary>
    /// Fills button with skill info
    /// </summary>
    /// <param name="skill">Skill to copy info from</param>
    /// <param name="tree">Reference to tree controller</param>
    public void Init(Skill skill, SkillTreeController tree)
    {
        skillTreeController = tree;
        nameText.text = skill.Name;
        skillSpriteImage.sprite = skill.Sprite;

        SetLearnedState(false);
    }

    /// <summary>
    /// Sets button's color according to skill state
    /// </summary>
    /// <param name="state">Is skill learned?</param>
    public void SetLearnedState(bool state)
    {
        skillFrame.color = state ? Color.green : Color.gray;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        skillTreeController.SelectSkill(SkillPlacementId);
    }
}
