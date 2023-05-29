
public class WeaponData 
{
	public int weaponid { get; }
	public string weaponname { get; }
    public string weapontype { get; }
    public int damage { get; }
    public float shootinterval { get; }
    public int nums { get;}
    public string modelpath { get; }
    public string imagepath { get; }

    public WeaponData(int weaponid, string weaponname, string weapontype, int damage,float shootinterval,int nums,
                     string modelpath, string imagepath)
    {
        this.weaponid = weaponid;
        this.weaponname = weaponname;
        this.weapontype = weapontype;
        this.damage = damage;
        this.shootinterval = shootinterval;
        this.nums = nums;
        this.modelpath = modelpath;
        this.imagepath = imagepath;
    }

}
