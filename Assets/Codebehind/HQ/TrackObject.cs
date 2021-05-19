using HQ;
using UnityEngine;


[CreateAssetMenu( fileName = "New Track",  menuName = "HQ/Track")]
public class TrackObject: ScriptableObject
{
    public int Length;
    public Line[] lines { get; protected set; }
    [HideInInspector]
    public TrackModifier[] Modifier;
    public float roadWidth;
    public int segmentLength;
    public float trackHeight;

    private void OnEnable()
    {
        Construct();
    }

    protected virtual void Construct()
    {
        Length = 1600;
        lines = new Line[Length];

        for (int i = 0; i < Length; i++)
        {
            ref Line line = ref lines[i];
            line.z = i * segmentLength;
            line.w = roadWidth;

            foreach (var m in Modifier)
            {
                if (!m.disabled && m.Segments.InRange(i) && i % m.frequency == 0)
                {
                    line.curve += m.curve;
                    line.spriteX = m.spriteX;
                    line.y += Mathf.Sin(i * m.h) * trackHeight;
                    line.sprite = m.sprite ?? line.sprite;
                    line.flipX = m.flipX;
                }
            }

            //if (i > 300 && i < 700) line.curve = 0.5f;
            //if (i > 1100) line.curve = -0.7f;

            //if (i < 300 && i % 20 == 0) { line.spriteX = -2.5f; line.sprite = sprites[0]; }
            //if (i % 17 == 0) { line.spriteX = 2.0f; line.sprite = sprites[1]; }
            //if (i > 300 && i % 20 == 0) { line.spriteX = -1.7f; line.sprite = sprites[2]; }
            //if (i > 800 && i % 20 == 0) { line.spriteX = -1.3f; line.sprite = sprites[3]; }
            //if (i == 400) { line.spriteX = -1.3f; line.sprite = sprites[4]; }

            //if (i > 750) line.y = Mathf.Sin(i / 30.0f) * trackHeight;
        }
    }

}

