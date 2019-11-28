﻿# 项目相关

## 常规

1. README请使用Markdown语法编写
2. IIS程序池，请在高级设置中设置32位应用程序为true
3. 网站授权，将授权码放入XmlConfig目录下的license.txt文件中，注意：请不要提交到svn

## 接口

本项目已经整合了许多开发 API 所必要的组件：

1. Microsoft.AspNet.WebApi: 微软官方接口框架
2. Microsoft.AspNet.Cors: 接口跨域
3. JWT：接口token授权

项目接口对于提交数据进行了MD5签名，当发布代码，以Release运行，则必须开启DataValidate验证配置
对于Debug与Release的区分，需要确保网站目录Properties/AssemblyInfo.cs发布相关文件下AssemblyConfiguration节点如下配置

```
//这里设置生成的运行环境
#if DEBUG 
[assembly: AssemblyConfiguration("Debug")]//测试环境
#else
[assembly: AssemblyConfiguration("Release")] //生产环境
#endif

```





















```
	////////////////////////////////////////////////////////////////////
	//                          _ooOoo_                               //
	//                         o8888888o                              //
	//                         88" . "88                              //
	//                         (| ^_^ |)                              //
	//                         O\  =  /O                              //
	//                      ____/`---'\____                           //
	//                    .'  \\|     |//  `.                         //
	//                   /  \\|||  :  |||//  \                        //
	//                  /  _||||| -:- |||||-  \                       //
	//                  |   | \\\  -  /// |   |                       //
	//                  | \_|  ''\---/''  |   |                       //
	//                  \  .-\__  `-`  ___/-. /                       //
	//                ___`. .'  /--.--\  `. . ___                     //
	//              ."" '<  `.___\_<|>_/___.'  >'"".                  //
	//            | | :  `- \`.;`\ _ /`;.`/ - ` : | |                 //
	//            \  \ `-.   \_ __\ /__ _/   .-` /  /                 //
	//      ========`-.____`-.___\_____/___.-`____.-'========         //
	//                           `=---='                              //
	//      ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^        //
	//         佛祖保佑       永无BUG     永不修改                    //
	////////////////////////////////////////////////////////////////////
```