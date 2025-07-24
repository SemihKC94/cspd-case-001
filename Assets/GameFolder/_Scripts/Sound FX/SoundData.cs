using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKC.SoundFX
{
    [CreateAssetMenu(fileName = nameof(SoundData), menuName = nameof(SKC) + "/New" + nameof(SoundData))]

    public class SoundData : ScriptableObject
    {
        [SerializeField] SoundClip[] soundClips;

        public SoundClip[] SoundClips => soundClips;
    }
}
