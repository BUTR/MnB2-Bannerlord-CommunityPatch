using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace CommunityPatch {

  public static class ViewModelExtensions {

    public static readonly AccessTools.FieldRef<ViewModel, Dictionary<string, PropertyInfo>> propertyInfosField = AccessTools.FieldRefAccess<ViewModel, Dictionary<string, PropertyInfo>>("_propertyInfos");

    public static bool TryAddCustomStaticProperty(this ViewModel viewModel, [NotNull] PropertyInfo propertyInfo, string name = null) {
      if (propertyInfo == null)
        throw new ArgumentNullException(nameof(propertyInfo));
      if (propertyInfo.GetMethod?.IsStatic ?? propertyInfo.SetMethod?.IsStatic ?? false)
        throw new ArgumentException("Expected static property.", nameof(propertyInfo));

      var t = propertyInfo.PropertyType;
      if (!typeof(ViewModel).IsAssignableFrom(t) && !typeof(IMBBindingList).IsAssignableFrom(t))
        throw new ArgumentException("Property must return a ViewModel or IMBBindingList.", nameof(propertyInfo));

      try {
        propertyInfosField(viewModel).Add(name ??= propertyInfo.Name, propertyInfo);
      }
      catch (ArgumentException) {
        return false;
      }

      return true;
    }

    private static bool TryAddCustomStaticProperty(this ViewModel viewModel, LambdaExpression propertyAccessExpression)
      => propertyAccessExpression.Body is MemberExpression m
        && (m.Member is PropertyInfo pi
          ? viewModel.TryAddCustomStaticProperty(pi)
          : throw new ArgumentException($"Expected a property, got {m.Member.MemberType}"));

    public static bool TryAddCustomStaticProperty(this ViewModel viewModel, Expression<Func<ViewModel>> propertyAccessExpression)
      => viewModel.TryAddCustomStaticProperty((LambdaExpression) propertyAccessExpression);

    public static bool TryAddCustomStaticProperty(this ViewModel viewModel, Expression<Func<IMBBindingList>> propertyAccessExpression)
      => viewModel.TryAddCustomStaticProperty((LambdaExpression) propertyAccessExpression);

  }

}