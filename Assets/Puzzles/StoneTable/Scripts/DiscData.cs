using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data")]
public class DiscData : ScriptableObject
{
    public Sprite glowSprite;
    public Sprite beamSprite;

    public Sprite enableSprite;
    public Sprite disableSprite;

    public int discCount;
    public float circleAngularOffset;
    public float circleDist;
    public List<float> circleAngles;
    public List<int> circleWinPos;

    public float buttonDist;
}
