﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Editor.UnitTests.SignatureHelp
Imports Microsoft.CodeAnalysis.Editor.UnitTests.Workspaces
Imports Microsoft.CodeAnalysis.SignatureHelp
Imports Microsoft.CodeAnalysis.VisualBasic.SignatureHelp

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.SignatureHelp
    Public Class FunctionAggregationSignatureHelpProviderTests
        Inherits AbstractVisualBasicSignatureHelpProviderTests

        Public Sub New(workspaceFixture As VisualBasicTestWorkspaceFixture)
            MyBase.New(workspaceFixture)
        End Sub

        Friend Overrides Function CreateSignatureHelpProvider() As ISignatureHelpProvider
            Return New FunctionAggregationSignatureHelpProvider()
        End Function

        <WorkItem(529682, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/529682")>
        <Fact(), Trait(Traits.Feature, Traits.Features.SignatureHelp)>
        Public Async Function TestAggregateFunctionInAggregateClause() As Task
            Dim markup = <Text><![CDATA[
Imports System.Linq

Module Program
    Sub Main(args As String())
        Dim lambda = Aggregate i In New Integer() {1} Into Count($$
    End Sub
End Module
]]></Text>.Value

            Dim expectedOrderedItems = New List(Of SignatureHelpTestItem)()
            expectedOrderedItems.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> Count() As Integer", String.Empty, Nothing, currentParameterIndex:=0))
            expectedOrderedItems.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> Count({VBWorkspaceResources.expression} As Boolean) As Integer", String.Empty, Nothing, currentParameterIndex:=0))

            Await TestAsync(markup, expectedOrderedItems)
        End Function

#Region "EditorBrowsable tests"
        <WorkItem(7336, "DevDiv_Projects/Roslyn")>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Async Function TestEditorBrowsable_FunctionAggregation_BrowsableStateAlways() As Task
            Dim markup = <Text><![CDATA[
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Linq

Class C
    Sub M()
        Dim numbers As IEnumerable(Of Integer)
        Dim query = Aggregate num In numbers Into GetRandomNumber($$
    End Sub
End Class
]]></Text>.Value

            Dim referencedCode = <Text><![CDATA[
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

Public Module Goo
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Always)>
    <Extension()>
    Public Function GetRandomNumber(ByVal values As IEnumerable(Of Integer)) As Integer
        Return 4
    End Function
End Module
]]></Text>.Value
            Dim expectedOrderedItems = New List(Of SignatureHelpTestItem)()
            expectedOrderedItems.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> GetRandomNumber() As Integer", String.Empty, Nothing, currentParameterIndex:=0))

            Await TestSignatureHelpInEditorBrowsableContextsAsync(markup:=markup,
                                                referencedCode:=referencedCode,
                                                expectedOrderedItemsMetadataReference:=expectedOrderedItems,
                                                expectedOrderedItemsSameSolution:=expectedOrderedItems,
                                                sourceLanguage:=LanguageNames.VisualBasic,
                                                referencedLanguage:=LanguageNames.VisualBasic)
        End Function

        <WorkItem(7336, "DevDiv_Projects/Roslyn")>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Async Function TestEditorBrowsable_FunctionAggregation_BrowsableStateNever() As Task
            Dim markup = <Text><![CDATA[
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Linq

Class C
    Sub M()
        Dim numbers As IEnumerable(Of Integer)
        Dim query = Aggregate num In numbers Into GetRandomNumber($$
    End Sub
End Class
]]></Text>.Value

            Dim referencedCode = <Text><![CDATA[
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

Public Module Goo
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
    <Extension()>
    Public Function GetRandomNumber(ByVal values As IEnumerable(Of Integer)) As Integer
        Return 4
    End Function
End Module
]]></Text>.Value
            Dim expectedOrderedItems = New List(Of SignatureHelpTestItem)()
            expectedOrderedItems.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> GetRandomNumber() As Integer", String.Empty, Nothing, currentParameterIndex:=0))

            Await TestSignatureHelpInEditorBrowsableContextsAsync(markup:=markup,
                                                referencedCode:=referencedCode,
                                                expectedOrderedItemsMetadataReference:=New List(Of SignatureHelpTestItem)(),
                                                expectedOrderedItemsSameSolution:=expectedOrderedItems,
                                                sourceLanguage:=LanguageNames.VisualBasic,
                                                referencedLanguage:=LanguageNames.VisualBasic)
        End Function

        <WorkItem(7336, "DevDiv_Projects/Roslyn")>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Async Function TestEditorBrowsable_FunctionAggregation_BrowsableStateAdvanced() As Task
            Dim markup = <Text><![CDATA[
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Linq

Class C
    Sub M()
        Dim numbers As IEnumerable(Of Integer)
        Dim query = Aggregate num In numbers Into GetRandomNumber($$
    End Sub
End Class
]]></Text>.Value

            Dim referencedCode = <Text><![CDATA[
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

Public Module Goo
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
    <Extension()>
    Public Function GetRandomNumber(ByVal values As IEnumerable(Of Integer)) As Integer
        Return 4
    End Function
End Module
]]></Text>.Value
            Dim expectedOrderedItems = New List(Of SignatureHelpTestItem)()
            expectedOrderedItems.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> GetRandomNumber() As Integer", String.Empty, Nothing, currentParameterIndex:=0))

            Await TestSignatureHelpInEditorBrowsableContextsAsync(markup:=markup,
                                                referencedCode:=referencedCode,
                                                expectedOrderedItemsMetadataReference:=New List(Of SignatureHelpTestItem),
                                                expectedOrderedItemsSameSolution:=expectedOrderedItems,
                                                sourceLanguage:=LanguageNames.VisualBasic,
                                                referencedLanguage:=LanguageNames.VisualBasic,
                                                hideAdvancedMembers:=True)

            Await TestSignatureHelpInEditorBrowsableContextsAsync(markup:=markup,
                                                referencedCode:=referencedCode,
                                                expectedOrderedItemsMetadataReference:=expectedOrderedItems,
                                                expectedOrderedItemsSameSolution:=expectedOrderedItems,
                                                sourceLanguage:=LanguageNames.VisualBasic,
                                                referencedLanguage:=LanguageNames.VisualBasic,
                                                hideAdvancedMembers:=False)
        End Function

        <WorkItem(7336, "DevDiv_Projects/Roslyn")>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Async Function TestEditorBrowsable_FunctionAggregation_BrowsableStateMixed() As Task
            Dim markup = <Text><![CDATA[
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Linq

Class C
    Sub M()
        Dim numbers As IEnumerable(Of Integer)
        Dim query = Aggregate num In numbers Into [|GetRandomNumber($$
    |]End Sub
End Class
]]></Text>.Value

            Dim referencedCode = <Text><![CDATA[
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System

Public Module Goo
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Always)>
    <Extension()>
    Public Function GetRandomNumber(ByVal values As IEnumerable(Of Integer)) As Integer
        Return 4
    End Function
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
    <Extension()>
    Public Function GetRandomNumber(Of T)(ByVal values As IEnumerable(Of T), ByVal selector As Func(Of T, Double)) As Integer
        Return 4
    End Function
End Module
]]></Text>.Value
            Dim expectedOrderedItemsMetadataOnly = New List(Of SignatureHelpTestItem)()
            expectedOrderedItemsMetadataOnly.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> GetRandomNumber() As Integer", String.Empty, Nothing, currentParameterIndex:=0))

            Dim expectedOrderedItemsSameSolution = New List(Of SignatureHelpTestItem)()
            expectedOrderedItemsSameSolution.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> GetRandomNumber() As Integer", String.Empty, Nothing, currentParameterIndex:=0))
            expectedOrderedItemsSameSolution.Add(New SignatureHelpTestItem($"<{VBFeaturesResources.Extension}> GetRandomNumber({VBWorkspaceResources.expression} As Double) As Integer", String.Empty, String.Empty, currentParameterIndex:=0))

            Await TestSignatureHelpInEditorBrowsableContextsAsync(markup:=markup,
                                                referencedCode:=referencedCode,
                                                expectedOrderedItemsMetadataReference:=expectedOrderedItemsMetadataOnly,
                                                expectedOrderedItemsSameSolution:=expectedOrderedItemsSameSolution,
                                                sourceLanguage:=LanguageNames.VisualBasic,
                                                referencedLanguage:=LanguageNames.VisualBasic)
        End Function
#End Region
    End Class
End Namespace
