# Phone UI Placeholders

The `PhoneController` exposes Inspector-assignable slots for future UI work:

- `phoneScreen` — root GameObject of the phone UI panel. Activated when a message is shown.
- `timeText` — `TMPro.TextMeshProUGUI` element that displays the current device time in `HH:mm` format.
- `quranSource` — `AudioSource` for Quran playback. No placeholder clip is assigned in the prototype; assign an `AudioClip` in the Inspector to enable `ToggleQuran()`.

These are intentionally left unwired in the greybox prototype. Phase 2 will add the actual Canvas, Text, and audio assets.
