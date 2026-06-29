using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("UI Sliders")]
    public Slider sliderMaster; // Slider untuk SEMUA suara
    public Slider sliderMusik;  // Slider untuk Musik (BGM)

    // Nama Parameter Mixer (Disamakan persis dengan yang di-expose di Unity Mixer)
    private string masterParam = "MasterParam"; 
    private string musikParam = "MusikParam";   

    void Start()
    {
        // 1. Atur batas minimum dan maksimum semua Slider UI (0.0001f agar tidak log10(0) yang menghasilkan error)
        sliderMaster.minValue = 0.0001f;
        sliderMaster.maxValue = 1f;
        sliderMusik.minValue = 0.0001f;
        sliderMusik.maxValue = 1f;

        // 2. Ambil data volume yang tersimpan (Default: 0.75f jika belum ada data)
        float savedMaster = PlayerPrefs.GetFloat("SavedMaster", 0.75f);
        float savedMusik = PlayerPrefs.GetFloat("SavedMusik", 0.75f);

        // 3. Terapkan nilai penyimpanan ke UI Slider
        sliderMaster.value = savedMaster;
        sliderMusik.value = savedMusik;

        // 4. Terapkan nilai ke Audio Mixer langsung saat game dimulai
        SetMaster(savedMaster);
        SetMusik(savedMusik);

        // 5. Menghubungkan semua Slider ke Fungsi secara otomatis melalui kode
        sliderMaster.onValueChanged.AddListener(SetMaster);
        sliderMusik.onValueChanged.AddListener(SetMusik);
    }

    // Fungsi untuk mengatur Master Volume (Semua suara)
    public void SetMaster(float value)
    {
        audioMixer.SetFloat(masterParam, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SavedMaster", value);
        PlayerPrefs.Save(); // Langsung simpan perubahan posisi slider
    }

    // Fungsi untuk mengatur volume Musik Latar (BGM)
    public void SetMusik(float value)
    {
        audioMixer.SetFloat(musikParam, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SavedMusik", value);
        PlayerPrefs.Save(); // Langsung simpan perubahan posisi slider
    }
}