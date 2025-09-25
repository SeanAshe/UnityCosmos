using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BaseOperatableEntity
{
    public GameObject arrowPrefab;
    public Transform guideTransform;
    public override GameObject ArrowPrefab => arrowPrefab;
    public override Transform GuideTransform => guideTransform;
}
