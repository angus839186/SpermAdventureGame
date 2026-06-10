using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MeiosisChromosomeView : MonoBehaviour
{
    [Header("染色體設定")]
    [SerializeField] private ChromosomeColor colorType = ChromosomeColor.Blue;
    [SerializeField] private ChromosomeLength length = ChromosomeLength.Long;
    [SerializeField] private bool isPair;
    [Header("染色體圖片")]
    [SerializeField] private Sprite armSprite;
    [Header("著絲點圖片")]
    [SerializeField] private Sprite centromereSprite;

    public ChromosomeColor ColorType => colorType;
    public ChromosomeLength Length => length;
    public bool IsPair => isPair;
    public Sprite ArmSprite => armSprite;
    public Sprite CentromereSprite => centromereSprite;

}
