# ng2 的创建命令

## 项目指令

1. ng new project-name - 创建一个新项目，置为默认设置
2. ng build - 构建/编译应用
3. ng test - 运行单元测试
4. ng e2e - 运行端到端（end-to-end）测试
5. ng serve - 启动一个小型web服务器，用于托管应用
6. ng deploy - 即开即用，部署到Github Pages或者Firebase

执行这些步骤所需要的全部设置，都由CLI工具来完成。

## 开发指令

除了设置一个新应用之外，该工具还支持开发者运行命令，构建应用的组成部分，如组件（Component）和路由（Route）。

1. ng generate component my-comp - 生成一个新组件，同时生成其测试规格和相应的HTML/CSS文件
2. ng generate directive my-directive - 生成一个新指令
3. ng generate pipe my-pipe - 生成一个新管道
4. ng generate service my-service - 生成一个新服务
5. ng generate route my-route - 生成一个新路由
6. ng generate class my-class - 生成一个简易的模型类