using UnityEngine;

public class Box : MonoBehaviour
{
    public string BoxName { set; get; }

    public void SetParent(Transform parentTrans)
    {
        //gameObject.transform.SetParent(boxSo.parentTransform);
    }

    public struct BoxData
    {
        public Mesh mesh; 
        public Material mat;
        public string name;
        public Item item;
    }

    public BoxData CaptureState()
    {
        BoxData data = new BoxData();
    
        data.mesh = GetComponent<MeshFilter>().mesh;
        data.mat = GetComponent<MeshRenderer>().material;
        data.name = gameObject.name;

    
        return data;
    }

    public void RestoreState(CarrierData state)
    {
        throw new System.NotImplementedException();
    }
}
