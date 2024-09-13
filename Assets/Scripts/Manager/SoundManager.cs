using UnityEngine;

public enum ClipType
{
    None,
    PlayCard,
    Shuffle,
    Winner,
    Loser,
    Last
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager self;
    [SerializeField]
    private AudioSource clipSource;
    [SerializeField]
    private AudioClip PlayCardClip;
    [SerializeField]
    private AudioClip ShuffleClip;
    [SerializeField]
    private AudioClip WinnerClip;
    [SerializeField]
    private AudioClip LoseClip;

    public SoundManager()
    {
        if (self == null)
        {
            self = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayClip(ClipType type = ClipType.None)
    {
        if (clipSource.isPlaying)
        {
            clipSource.Stop();
        }

        switch (type)
        {
            case ClipType.None:
                break;
            case ClipType.PlayCard:
                clipSource.PlayOneShot(PlayCardClip);
                break;
            case ClipType.Shuffle:
                clipSource.PlayOneShot(ShuffleClip);
                break;
            case ClipType.Winner:
                clipSource.PlayOneShot(WinnerClip);
                break;
            case ClipType.Loser:
                clipSource.PlayOneShot(LoseClip);
                break;
            case ClipType.Last:
                break;
            default:
                break;
        }
    }
}
