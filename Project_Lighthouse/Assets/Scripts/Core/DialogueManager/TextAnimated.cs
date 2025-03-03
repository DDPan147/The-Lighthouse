using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Text that can supply a custom shader with data for animated text.
/// </summary>
public class TextAnimated : Text
{
    private float[] animateVerts = new float[16]; // 16 is arbitrarily large. Make sure this matches the shader.

    [SerializeField]
    private Material baseMaterial;
    private const char delimiter = '`';

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public void Start()
#pragma warning restore CS0114 // Unity will handle that since this is a Unity function.
    {
        this.material = Material.Instantiate(this.baseMaterial); // Make a copy so we don't modify the base material. Note that this means edits to the shader won't affect this while in Play mode.
        this.SetText("Here's some text. `WOW!!!` It's great!\nLook, it even crosses `multiple\nlines!`"); // Just for demonstration.
    }

    /// <summary>
    /// Be sure to call this instead of setting TextAnimated.text!
    /// </summary>
    public void SetText(string newText)
    {
        // If the text uses the delimiter, we need to both calculate the correct vertex indices and remove the delimiter from the output text.
        if (newText.Contains(delimiter))
        {
            string[] substrings = newText.Split(delimiter);
            int charCount = 0;
            int spaces = 0; // Whitespace doesn't have a glyph, so we need to deduct from the vertex indices when counting characters.
            StringBuilder output = new StringBuilder(); // The actual output text should not have the delimiter.

            for (int s = 0; s < substrings.Length; s++)
            {
                output.Append(substrings[s]);
                if (s == substrings.Length - 1 && s % 2 == 0) // The text to animate will always be an odd-numbered substring,
                    break;                                    // so if we're on an even-numbered substring with no corresponding odd-numbered one, we can just stop.

                spaces += substrings[s].Count(c => char.IsWhiteSpace(c));

                this.animateVerts[s] = charCount + substrings[s].Length - spaces; // This gives the index of the character at the start/end of an animated text region, accounting for whitespace.
                this.animateVerts[s] =
                    this.animateVerts[s] * 4 + // Each glyph has 4 vertices (TL->TR->BR->BL), so this gives the actual vertex index.
                    (s % 2 == 1 ? -1 : 0); // For the ends of animated text substrings, that index will be the first index after the substring, so add -1 to get the last index of the substring instead.

                charCount += substrings[s].Length;
            }
            this.animateVerts[substrings.Length] = -1; // We'll use a -1 index so the shader knows where the valid data ends, since we don't ever clear out this array.
            this.text = output.ToString();
        }
        // If the text doesn't use the delimiter, just show it normally.
        else
        {
            this.animateVerts[0] = -1;
            this.text = newText;
        }
    }

    /// <summary>
    /// This is called whenever the text Graphic needs to be redrawn. We're just using it to know when to update the shader data.
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        this.material.SetFloatArray("_AnimateVerts", this.animateVerts);
    }
}
