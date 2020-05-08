using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  [Guid("BA3FEE4C-ECB9-4e41-83B7-183FA41CD859"), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataEmit {

    void SetModuleProps([MarshalAs(UnmanagedType.LPWStr)] string szName);

    void Save([MarshalAs(UnmanagedType.LPWStr)] string szFile, uint dwSaveFlags);

    void SaveToStream([MarshalAs(UnmanagedType.Interface)] object pIStream, uint dwSaveFlags);

    void GetSaveSize(CorSaveSize fSave, out uint pdwSaveSize);

    void DefineTypeDef([MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, uint dwTypeDefFlags, uint tkExtends, [MarshalAs(UnmanagedType.LPArray)] uint[] rtkImplements, out uint ptd);

    void DefineNestedType([MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, uint dwTypeDefFlags, uint tkExtends, [MarshalAs(UnmanagedType.LPArray)] uint[] rtkImplements, uint tdEncloser, out uint ptd);

    void SetHandler([MarshalAs(UnmanagedType.IUnknown)] object pUnk);

    void DefineMethod(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, MethodAttributes dwMethodFlags,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pvSigBlob, uint cbSigBlob, uint ulCodeRVA, MethodImplAttributes dwImplFlags, out uint pmd);

    void DefineMethodImpl(uint td, uint tkBody, uint tkDecl);

    void DefineTypeRefByName(uint tkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] string szName, out uint ptr);

    void DefineImportType([MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyImport pAssemImport, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, [MarshalAs(UnmanagedType.Interface)] IMetaDataImport pImport, uint tdImport, [MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyEmit pAssemEmit, out uint ptr);

    void DefineMemberRef(uint tkImport, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pvSigBlob, uint cbSigBlob, out uint pmr);

    void DefineImportMember([MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyImport pAssemImport, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, [MarshalAs(UnmanagedType.Interface)] IMetaDataImport pImport, uint mbMember, [MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyEmit pAssemEmit, uint tkParent, out uint pmr);

    void DefineEvent(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szEvent, uint dwEventFlags, uint tkEventType, uint mdAddOn, uint mdRemoveOn, uint mdFire, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods, out uint pmdEvent);

    void SetClassLayout(uint td, uint dwPackSize, [MarshalAs(UnmanagedType.LPArray)] long[] rFieldOffsets, uint ulClassSize);

    void DeleteClassLayout(uint td);

    void SetFieldMarshal(uint tk, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pvNativeType, uint cbNativeType);

    void DeleteFieldMarshal(uint tk);

    void DefinePermissionSet(uint tk, uint dwAction, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pvPermission, uint cbPermission, out uint ppm);

    void SetRVA(uint md, uint ulRVA);

    void GetTokenFromSig([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      byte[] pvSig, uint cbSig, out uint pmsig);

    void DefineModuleRef([MarshalAs(UnmanagedType.LPWStr)] string szName, out uint pmur);

    void SetParent(uint mr, uint tk);

    void GetTokenFromTypeSpec([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      byte[] pvSig, uint cbSig, out uint ptypespec);

    void SaveToMemory(IntPtr pbData, uint cbData);

    void DefineUserString([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)]
      string szString, uint cchString, out uint pstk);

    void DeleteToken(uint tkObj);

    void SetMethodProps(uint md, uint dwMethodFlags, uint ulCodeRVA, uint dwImplFlags);

    void SetTypeDefProps(uint td, uint dwTypeDefFlags, uint tkExtends, [MarshalAs(UnmanagedType.LPArray)] uint[] rtkImplements);

    void SetEventProps(uint ev, uint dwEventFlags, uint tkEventType, uint mdAddOn, uint mdRemoveOn, uint mdFire, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods);

    void SetPermissionSetProps(uint tk, uint dwAction, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pvPermission, uint cbPermission, out uint ppm);

    void DefinePinvokeMap(uint tk, uint dwMappingFlags, [MarshalAs(UnmanagedType.LPWStr)] string szImportName, uint mrImportDLL);

    void SetPinvokeMap(uint tk, uint dwMappingFlags, [MarshalAs(UnmanagedType.LPWStr)] string szImportName, uint mrImportDLL);

    void DeletePinvokeMap(uint tk);

    void DefineCustomAttribute(uint tkObj, uint tkType, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pCustomAttribute, uint cbCustomAttribute, out uint pcv);

    void SetCustomAttributeValue(uint pcv, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pCustomAttribute, uint cbCustomAttribute);

    void DefineField(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwFieldFlags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pvSigBlob, uint cbSigBlob, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)]
      byte[] pValue, uint cchValue, out uint pmd);

    void DefineProperty(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szProperty, uint dwPropFlags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pvSig, uint cbSig, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)]
      byte[] cvalue, uint cchValue, uint mdSetter, uint mdGetter, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods, out uint pmdProp);

    void DefineParam(uint md, uint ulParamSeq, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwParamFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)]
      byte[] pValue, uint cchValue, out uint ppd);

    void SetFieldProps(uint fd, uint dwFieldFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pValue, uint cchValue);

    void SetPropertyProps(uint pr, uint dwPropFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pValue, uint cchValue, uint mdSetter, uint mdGetter, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods);

    void SetParamProps(uint pd, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwParamFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      byte[] pValue, uint cchValue);

    void DefineSecurityAttributeSet(uint tkObj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      COR_SECATTR[] rSecAttrs, uint cSecAttrs, out uint pulErrorAttr);

    void ApplyEditAndContinue([MarshalAs(UnmanagedType.IUnknown)] object pImport);

    void TranslateSigWithScope([MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyImport pAssemImport, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, [MarshalAs(UnmanagedType.Interface)] IMetaDataImport import, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      byte[] pbSigBlob, uint cbSigBlob, [MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyEmit pAssemEmit, [MarshalAs(UnmanagedType.Interface)] IMetaDataEmit emit, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 9)]
      byte[] pvTranslatedSig, uint cbTranslatedSigMax, out uint pcbTranslatedSig);

    void SetMethodImplFlags(uint md, uint dwImplFlags);

    void SetFieldRVA(uint fd, uint ulRVA);

    void Merge([MarshalAs(UnmanagedType.Interface)] IMetaDataImport pImport, [MarshalAs(UnmanagedType.Interface)] IMapToken pHostMapToken, [MarshalAs(UnmanagedType.IUnknown)] object pHandler);

    void MergeEnd();

  };

}