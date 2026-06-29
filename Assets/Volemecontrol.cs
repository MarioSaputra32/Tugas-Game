using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("UI Sliders")]
    public Slider sliderMaster; // Slider untuk SEMUA suara
    public Slider sliderVolume; // Slider untuk Efek Suara (SFX)
    public Slider sliderMusik;  // Slider untuk Musik (BGM)

    [Header("Nama Parameter Mixer (Harus Sama Persis)")]
    private string masterParam = "MasterParam";
    private string volumeParam = "VolumeParam"; 
    private string musikParam = "MusikParam";   

    void Start()
    {
        // 1. Atur batas minimum dan maksimum semua Slider UI
        sliderMaster.minValue = 0.0001f;
        sliderMaster.maxValue = 1f;
        sliderVolume.minValue = 0.0001f;
        sliderVolume.maxValue = 1f;
        sliderMusik.minValue = 0.0001f;
        sliderMusik.maxValue = 1f;

        // 2. Ambil data volume yang tersimpan (Default: 0.75f jika belum ada data)
        float savedMaster = PlayerPrefs.GetFloat("SavedMaster", 0.75f);
        float savedVolume = PlayerPrefs.GetFloat("SavedVolume", 0.75f);
        float savedMusik = PlayerPrefs.GetFloat("SavedMusik", 0.75f);

        // 3. Terapkan nilai penyimpanan ke Slider UI
        sliderMaster.value = savedMaster;
        sliderVolume.value = savedVolume;
        sliderMusik.value = savedMusik;

        // 4. Terapkan nilai ke Audio Mixer langsung saat game dimulai
        SetMaster(savedMaster);
        SetVolume(savedVolume);
        SetMusik(savedMusik);

        // 5. Menghubungkan semua Slider ke Fungsi via Code secara otomatis
        sliderMaster.onValueChanged.AddListener(SetMaster);
        sliderVolume.onValueChanged.AddListener(SetVolume);
        sliderMusik.onValueChanged.AddListener(SetMusik);
    }

    // Fungsi untuk mengatur ALL/MASTER Volume (Semua suara)
    public void SetMaster(float value)
    {
        audioMixer.SetFloat(masterParam, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SavedMaster", value);
    }

    // Fungsi untuk mengatur volume SFX / Efek Suara Umum
    public void SetVolume(float value)
    {
        audioMixer.SetFloat(volumeParam, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SavedVolume", value);
    }

    // Fungsi untuk mengatur volume Musik Latar (BGM)
    public void SetMusik(float value)
    {
        audioMixer.SetFloat(musikParam, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SavedMusik", value);
    }

    // Dipanggil otomatis oleh Unity saat game keluar/ganti scene agar data tersimpan aman
    void OnDisable()
    {
        PlayerPrefs.Save();
    }
}