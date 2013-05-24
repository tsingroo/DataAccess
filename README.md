DataAccess
==========

c# 通用数据访问类库
daEntity为实体类，本质上为键值对，存储一个实体对象的列名和值

假设有表 test ,有两列id,val

初始化表daTable testTable = new daTable("connectionString","test","id");


新增Insert方法
daEntity en = new daEntity();
en.Add("id","1");
en.Add("val","0001");
daEntity en2 = new daEntity();
en2.Add("id","2");
en2.Add("val","0002");
List<daEntity> enList = new List<daEntity>();
enList.Add(en);
enList.Add(en2);
int affectedRows = testTable.Insert(enList);--新增,返回受影响行数,只在2008和以后的数据库才能成功.


修改Update方法
en =new daEntity();
en.add("id","1");//必须添加主键的值,根据主键修改
en.add("val","XXXX");
affectedRows = testTable.Update(en);--修改，返回受影响行数


删除Delete方法
en =new daEntity();
en.add("id","1");
affectedRows = testTable.Delete(en);--删除返回受影响行数
--删除方法可以根据其他条件删除
en =new daEntity();
en.add("val","XXXX");
affectedRows = testTable.Delete(en);--删除


获取数据GetRows方法
en =new daEntity();
en.add("val","XXXX");
string[] columns=new stinrg[]{"id", "val"};
SsqlDataReader reader = testTable.GetRows(en,columns);
while(reader.Read()) {
	//获取数据
}
