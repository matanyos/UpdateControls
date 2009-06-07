/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
	internal abstract class ObjectPropertyCollection : ObjectProperty
	{
		private ObservableCollection<object> _collection = new ObservableCollection<object>();
        private Dependent _depCollection;

        public ObjectPropertyCollection(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
            if (ClassProperty.CanRead)
            {
                // When the collection is out of date, update it from the wrapped object.
                _depCollection = new Dependent(OnUpdateCollection);

                // When the property becomes out of date, trigger an update.
                _depCollection.Invalidated += TriggerUpdate;

                // The property is out of date right now, so trigger the first update.
                TriggerUpdate();
            }
        }

        private void OnUpdateCollection()
        {
            // Get the source collection from the wrapped object.
            IEnumerable sourceCollection = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject) as IEnumerable;

            // Create a list of new items.
            List<CollectionItem> items = new List<CollectionItem>();

            // Dump all previous items into a recycle bin.
            using (RecycleBin<CollectionItem> bin = new RecycleBin<CollectionItem>())
            {
                foreach (object oldItem in _collection)
                    bin.AddObject(new CollectionItem(_collection, oldItem, true));
                // Add new objects to the list.
                if (sourceCollection != null)
                    foreach (object obj in sourceCollection)
                        items.Add(bin.Extract(new CollectionItem(_collection, TranslateOutgoingValue(obj), false)));
                // All deleted items are removed from the collection at this point.
            }
            // Ensure that all items are added to the list.
            int index = 0;
            foreach (CollectionItem item in items)
            {
                item.EnsureInCollection(index);
                ++index;
            }
        }

        private void TriggerUpdate()
        {
            ObjectInstance.Dispatcher.BeginInvoke(new Action(delegate
            {
                _depCollection.OnGet();
            }));
        }

        public override void OnUserInput(object value)
		{
			throw new NotSupportedException("Update Controls does not support two-way binding of collection properties.");
		}

        public override object Value
        {
            get { return _collection; }
        }

        public abstract object TranslateOutgoingValue(object value);
    }
}