
namespace UpdateControls.UnitTest
{
	public class SourceData
	{
		private int _sourceProperty;

		#region Independent properties
		// Generated by Update Controls --------------------------------
		private Independent _indSourceProperty = new Independent();

		public int SourceProperty
		{
			get { _indSourceProperty.OnGet(); return _sourceProperty; }
			set { _indSourceProperty.OnSet(); _sourceProperty = value; }
		}
		// End generated code --------------------------------
		#endregion
	}
}