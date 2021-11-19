using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectsEnum { CapturePiece, DiceRoll, PickupPiece, PlacePiece }
public class SoundHandler : MonoBehaviour {
  public AudioClip CapturePiece;
  public AudioClip DiceRoll;
  public AudioClip Pickup;
  public AudioClip PlacePiece;

  public void PlaySound(SoundEffectsEnum soundEffect) {
    switch (soundEffect) {
      case SoundEffectsEnum.CapturePiece:
        GetComponent<AudioSource>().PlayOneShot(CapturePiece);
        break;
      case SoundEffectsEnum.DiceRoll:
        GetComponent<AudioSource>().PlayOneShot(DiceRoll);
        break;
      case SoundEffectsEnum.PickupPiece:
        GetComponent<AudioSource>().PlayOneShot(Pickup);
        break;
      case SoundEffectsEnum.PlacePiece:
        GetComponent<AudioSource>().PlayOneShot(PlacePiece);
        break;
    }
  }
}
