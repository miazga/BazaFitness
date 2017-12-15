package md5e73181269fb4fb5830d4689b9794c4df;


public class RecipeWrapper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("bazafitness.DAL.RecipeWrapper, bazafitness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RecipeWrapper.class, __md_methods);
	}


	public RecipeWrapper ()
	{
		super ();
		if (getClass () == RecipeWrapper.class)
			mono.android.TypeManager.Activate ("bazafitness.DAL.RecipeWrapper, bazafitness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
