using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolder : MonoBehaviour
{
    GameManager gameManager;

    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;

    [Header("WeaponSway Settings")]
    public float swayMultiplier;
    public float swaySmoothness;

    public Rigidbody playerRB;
    List<GameObject> weapons = new List<GameObject>();
    public GameObject currWeapon;
    [SerializeField] LayerMask enemyLayer;
    public Transform cam;
    public GameObject splashText;

    // Weapon Switcher UI
    Transform weaponSwitcher;

    private void Start() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        weaponSwitcher = GameObject.Find("MainCanvas").transform.Find("ConstantCanvas").transform.Find("HUD").Find("WeaponSwitcher");
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();

        RefreshWeaponList();
    }

    private void Update() {
        if (!gameManager.isActive) return;
        WeaponSwayFunc();
        SwitcherInput();
    }

    void WeaponSwayFunc() {
        mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        Quaternion Xrotation = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion Yrotation = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = Xrotation * Yrotation;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swaySmoothness * Time.deltaTime);
    }

    void RefreshWeaponList() {
        weapons.Clear();
        foreach (Transform t in transform) {
            if (t.GetComponent<GunScript>() || t.GetComponent<MeleeWeapon>())
                weapons.Add(t.gameObject);
        }
        currWeapon = weapons[0];
        RefreshWeapons();
    }

    void SwitcherInput() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            currWeapon = weapons[0];
            RefreshWeapons();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count > 0) {
            currWeapon = weapons[1];
            RefreshWeapons();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count > 1) {
            currWeapon = weapons[2];
            RefreshWeapons();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && weapons.Count > 2) {
            currWeapon = weapons[3];
            RefreshWeapons();
        }
    }

    void RefreshWeapons() {
        for (int i = 0; i < weapons.Count; i++) {
            if (weaponSwitcher.childCount > i) {
                if (weapons[i].GetComponent<GunScript>())
                    weaponSwitcher.transform.GetChild(i).GetComponent<Image>().sprite = weapons[i].GetComponent<GunScript>().weaponSprite;
                else
                    weaponSwitcher.transform.GetChild(i).GetComponent<Image>().sprite = weapons[i].GetComponent<MeleeWeapon>().meleeData.weaponSprite;
            }
            else {
                GameObject newWeaponSwitcherImage = Instantiate(new GameObject("Weapon" + i), transform);
                newWeaponSwitcherImage.transform.parent = weaponSwitcher;
                newWeaponSwitcherImage.transform.localPosition = new Vector3(0, 150 * (i - 1), 0);
                newWeaponSwitcherImage.AddComponent<Image>();
                i--;
                continue;
            }
            
            if (currWeapon == weapons[i]) {
                transform.GetChild(i).gameObject.SetActive(true);
                weaponSwitcher.transform.GetChild(i).gameObject.SetActive(true);
                weaponSwitcher.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else {
                transform.GetChild(i).gameObject.SetActive(false);
                weaponSwitcher.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                // weaponSwitcher.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
