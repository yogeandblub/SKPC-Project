# SKPC-Project — Unity App

A lightweight Unity sample showing how to **import humanoid avatars**, wire up **Animator Controllers** for dancing, **swap music** on button press, and keep a **camera follow** shot that works on **mobile**.

---

## ✨ Features

* **Two avatars** (Santa & Deer) imported as Humanoid with retargeted dance clips
* **Animator Controller** with Idle → Dance state machine + trigger
* **UI “Dance” button** that starts the dance and swaps background music
* **Camera follow** that frames the pair while they move
* **Mobile-ready**: touch UI, safe areas, aspect handling, and build profiles for iOS/Android

---

## 🧱 Tech & Requirements

* **Unity**: 2021.3 LTS or newer (tested up to 2022/2023 LTS)
* **Render Pipeline**: Built-in (URP compatible with minor setup)
* **Input**: Unity UI Button (no special input package required)
* **Audio**: 2× `AudioClip` (Idle loop + Dance track)
* **Target Platforms**: iOS, Android, Windows/macOS (editor safe)

---

## 📁 Project Structure

```
Assets/
  Animations/
    Santa/
    Deer/
    DanceClips/
  Art/
    Avatars/ (FBX/GLB, textures, materials)
  Audio/
    bg_idle.mp3
    bg_dance.mp3
  Prefabs/
    Santa.prefab
    Deer.prefab
  Scenes/
    Main.unity
  Scripts/
    DanceController.cs
    MusicSwitcher.cs
    FollowCamera.cs
  UI/
    Canvas.prefab
    DanceButton.prefab
  Settings/
    (quality, player, URP if used)
```

---

## 🚀 Quick Start

1. **Open** `Scenes/Main.unity`.
2. In **Hierarchy**, confirm you have:

   * `Santa (Prefab)` with `Animator`
   * `Deer (Prefab)` with `Animator`
   * `GameController` with `DanceController` + `MusicSwitcher`
   * `Main Camera` with `FollowCamera`
   * `Canvas` with **Dance** `Button` (OnClick → `DanceController.TriggerDance()`).
3. Enter **Play Mode** → Click **Dance**:

   * Both avatars start dancing.
   * Music switches to the dance track.
   * Camera follows the action.

---

## 🧍 Avatar Import (Humanoid)

1. Drag your **FBX/GLB** into `Assets/Art/Avatars/`.
2. Select the file → **Rig** tab → **Animation Type: Humanoid** → **Apply**.
3. Open **Configure…** to map bones if needed (head, spine, limbs).
4. For dance clips: import separate FBX/clip(s) into `Assets/Animations/DanceClips/`, also set **Humanoid** and **Loop Time/Loop Pose** if you want seamless loops.

---

## 🎛️ Animator Controller

Create a shared controller (per avatar or shared if retargetable):

1. `Create > Animator Controller` → `AC_Dancer`.
2. Add states:

   * **Idle** (default), set an idle clip.
   * **Dance** with your dance animation.
3. Add **Trigger parameter**: `DanceTrigger`.
4. Transition **Idle → Dance**: Condition = `DanceTrigger` (no Exit Time).
   Optional: add a **Blend Tree** if using multiple dances.

Assign `AC_Dancer` to both avatars’ `Animator`.

---

## 🕹️ UI Hookup

* Create a **Canvas** (Screen Space - Overlay), add a **Button** named **DanceButton**.
* On the Button’s **OnClick()**, drag the `GameController` and select:

  * `DanceController.TriggerDance()`

**Script: `DanceController.cs`**

```csharp
using UnityEngine;

public class DanceController : MonoBehaviour
{
    [SerializeField] private Animator santaAnimator;
    [SerializeField] private Animator deerAnimator;
    [SerializeField] private string danceTriggerName = "DanceTrigger";

    public void TriggerDance()
    {
        if (santaAnimator) santaAnimator.SetTrigger(danceTriggerName);
        if (deerAnimator)  deerAnimator.SetTrigger(danceTriggerName);

        // optional: notify music switcher
        var switcher = GetComponent<MusicSwitcher>();
        if (switcher) switcher.SwitchToDance();
    }
}
```

---

## 🎵 Music Switching

**Setup**

* Add an `AudioSource` to `GameController`, uncheck **Play On Awake**.
* Assign **Idle** and **Dance** clips in the inspector.

**Script: `MusicSwitcher.cs`**

```csharp
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSwitcher : MonoBehaviour
{
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip danceClip;
    [SerializeField] [Range(0f, 2f)] private float crossfadeTime = 0.6f;

    private AudioSource _src;
    private float _targetVol;
    private float _timer;
    private bool _crossfading;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        _src.loop = true;
        _src.clip = idleClip;
        _src.volume = 1f;
        _src.Play();
    }

    public void SwitchToDance() => CrossfadeTo(danceClip);
    public void SwitchToIdle()  => CrossfadeTo(idleClip);

    private void CrossfadeTo(AudioClip clip)
    {
        if (clip == null || _src.clip == clip) return;
        StartCoroutine(CrossfadeRoutine(clip));
    }

    private System.Collections.IEnumerator CrossfadeRoutine(AudioClip next)
    {
        float startVol = _src.volume;
        float t = 0f;
        while (t < crossfadeTime)
        {
            t += Time.deltaTime;
            _src.volume = Mathf.Lerp(startVol, 0f, t / crossfadeTime);
            yield return null;
        }
        _src.clip = next;
        _src.Play();
        t = 0f;
        while (t < crossfadeTime)
        {
            t += Time.deltaTime;
            _src.volume = Mathf.Lerp(0f, startVol, t / crossfadeTime);
            yield return null;
        }
    }
}
```

---

## 🎥 Camera Follow

Frames both avatars, keeping them in view during the dance.

**Script: `FollowCamera.cs`**

```csharp
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform santa;
    [SerializeField] private Transform deer;
    [SerializeField] private Vector3 offset = new Vector3(0, 3.5f, -6f);
    [SerializeField] private float smooth = 6f;
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 10f;

    void LateUpdate()
    {
        if (!santa || !deer) return;

        Vector3 mid = (santa.position + deer.position) * 0.5f;
        float dist = Mathf.Clamp(Vector3.Distance(santa.position, deer.position), minDistance, maxDistance);

        // Back up slightly as they spread apart
        Vector3 dynamicOffset = offset + new Vector3(0, 0, -0.3f * (dist - minDistance));

        Vector3 targetPos = mid + dynamicOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);
        transform.LookAt(mid + Vector3.up * 1.2f);
    }
}
```

---

## 📱 Mobile Adaptation Checklist

* **Canvas Scaler**:

  * `UI Scale Mode = Scale With Screen Size`
  * Reference Resolution e.g. `1080×1920`, Match = `0.5`
* **Safe Areas** (iOS/Android notches): Wrap UI in a `SafeArea` layout group or script.
* **Aspect Handling**: Test 16:9, 19.5:9, 4:3. Ensure avatars remain visible (tune `FollowCamera.offset`).
* **Quality Settings**: Medium to High; enable **GPU Instancing** on materials where possible.
* **Touch Targets**: Minimum 44–48 dp for buttons.
* **Audio**: Set `Prepare iOS for Recording` off unless required; verify background audio behavior.

---

## 🛠️ Build Instructions

### Android

1. **File → Build Settings → Android** (switch platform).
2. **Player Settings**:

   * `Minimum API Level`: 23+
   * `Target`: Highest installed
   * `Orientation`: Portrait or Auto-Rotation
3. Add `Main.unity` to **Scenes In Build**, then **Build** or **Build & Run**.

### iOS

1. **File → Build Settings → iOS** (switch platform).
2. **Player Settings**:

   * `Bundle Identifier`: `com.yourcompany.santadance`
   * `Orientation`: Portrait or Auto-Rotation
3. **Build**, open in Xcode, set team & signing, then run on device.

---

## 🔧 Troubleshooting

* **Avatars not moving**:

  * Animator Controller assigned? Humanoid rig set? `DanceTrigger` spelled correctly?
* **Music doesn’t change**:

  * `MusicSwitcher` present? Audio clips assigned? Check console for null refs.
* **Camera not following**:

  * Assign Santa & Deer transforms to `FollowCamera`. Verify they exist at runtime.
* **Clipping or feet sliding**:

  * In clip import, toggle **Foot IK** or adjust root motion (enable/disable on Animator).
* **UI not scaling on phones**:

  * Verify **Canvas Scaler** and Safe Area handling.

---

## 🧪 Optional: Multiple Dances

* Add parameters: `DanceIndex (int)`
* Use a **Blend Tree** or multiple **Dance** states with transitions on `DanceIndex`.
* Modify `DanceController` to set a random index before triggering.

---

## ✅ Roadmap / TODO

* [ ] Add second “Stop”/“Idle” button to return to idle track
* [ ] Add intro camera dolly or Cinemachine virtual camera
* [ ] Add FX (confetti/particles) on dance start
* [ ] Settings panel for volume, camera distance, quality

---

## 📜 License

MIT (replace with your preferred license).

---

## 🙌 Credits

* Avatars & animations: your sources here (note licenses)
* Music: your tracks here (note licenses)

---

## 📝 Copy-Paste Summary (Inspector Wiring)

* `Santa (Animator)`: **Controller = AC_Dancer**
* `Deer (Animator)`: **Controller = AC_Dancer**
* `GameController`:

  * **DanceController**: assign `santaAnimator`, `deerAnimator` (from the two prefabs)
  * **MusicSwitcher**: assign `idleClip`, `danceClip`; ensure `AudioSource` present
* `Main Camera`:

  * **FollowCamera**: assign Santa & Deer `Transform`
* `Canvas/Button (Dance)`:

  * OnClick → `GameController.DanceController.TriggerDance()`

Happy dancing! 💃🦌🎅

