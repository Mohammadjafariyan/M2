
using Models;

using System.Linq;
using Models;
using ServiceLayer.Systems;
namespace ServiceLayer.library
{
/// <summary>
    /// BookService
    /// BookService
    /// </summary>
    public class BookService :CommonService<Book>
    { public IQueryable<> GetAll(long @Id) {var dt=EngineContext.Database.SqlQuery<>(" DECLARE @Id bigintFalse;

  DECLARE   @Id bigint;    
    
    select  User.
      Id  
      as  
       [کد کاربر]  from User as
     _User  where  @Id <= User.[Id]");var res=dt.AsQueryable();return res; 
 }}}