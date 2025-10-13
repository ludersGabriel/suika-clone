using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Scriptable Objects/FruitData")]
public class FruitData : ScriptableObject {
  [SerializeField] private int id = 0;
  [SerializeField] private Sprite sprite;
  [SerializeField] private int points = 1;


  public Sprite Sprite => sprite;
  public int Points => points;

  public int Id => id;
}
