package md52b54e12525c028d30d81c3bd14720041;


public class ProductWrapper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("bazafitness.ProductWrapper, bazafitness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ProductWrapper.class, __md_methods);
	}


	public ProductWrapper ()
	{
		super ();
		if (getClass () == ProductWrapper.class)
			mono.android.TypeManager.Activate ("bazafitness.ProductWrapper, bazafitness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
