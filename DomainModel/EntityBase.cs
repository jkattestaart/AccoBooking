//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Runtime.Serialization;
using IdeaBlade.Core.DomainServices;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
  [RequiresAuthentication]
  [ClientCanSave(true)]
  public abstract partial class EntityBase
  {

    public virtual void Validate(VerifierResultCollection validationErrors)
    {
    }

  }

  [DataContract(IsReference = true)]
  [ClientCanSave(true)]
  public abstract class AuditEntityBase : EntityBase
  {
    [DataMember]
    public DateTime Created { get; set; }

    [DataMember]
    public string CreatedUser { get; set; }

    [DataMember]
    public DateTime Modified { get; set; }

    [DataMember]
    public string ModifyUser { get; set; }
  }

}