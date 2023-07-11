using UnityEngine;

[CreateAssetMenu(fileName = "Box", menuName = "Cargo/Box", order = 1)]
public class BoxSO : ScriptableObject
{
    public Mesh mesh; 
    public Material mat;
}