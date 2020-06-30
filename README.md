# ScanLineWithStencil

## 版本信息

unity版本2018.4.22f1

SampleScene场景



![image-20200531123532849](https://i.imgur.com/F3eHU0W.png)

## 运行步骤

1、output文件夹下有打包好的ScanLine.exe文件

![image-20200531121543698](https://i.imgur.com/LVecypI.png)

2、操作方式

wasd移动，shift快速按翻滚，按住奔跑，鼠标左右键 攻击防御

v键开启扫描线,扫描一定范围，扫到后高亮显示敌方

![](https://i.imgur.com/qGcR19P.png)

![](https://i.imgur.com/0tAvvTN.png)



3、present文件夹下有演示视频

![](https://i.imgur.com/COHUVYM.png)

## 技术说明

1、基本扫描线原理

每帧传入扫描范围的颜色叠加后处理特效，添加了一个非线性颜色曲线和远距离消散的效果

2、基于深度的3维重建

根据摄像机位置+摄像机在该像素的方向向量*像素的深度推算出像素的世界坐标，基于此可以完成以人物为中心的扩散式扫描线

3、基于模板缓冲的局部后处理

两次blit，第一次将帧的stencil buffer写进一张贴图中，stencil buffer的写入工作在player模型渲染时完成，这个方法可以用在很多模型上让其不备后处理影响

第二次得到以stencilTex传递的模板缓冲区，可以更加自由的手动完成模板测试



